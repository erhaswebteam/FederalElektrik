using FluentValidation;
using Grand.Core.Domain.Common;
using Grand.Framework.Validators;
using Grand.Services.Directory;
using Grand.Services.Localization;
using Grand.Web.Models.Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;

namespace Grand.Web.Validators.Common
{
    public class AddressValidator : BaseGrandValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.FirstName.Required"));
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.LastName.Required"));
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.Email.Required"));
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Common.WrongEmail"));
            if (addressSettings.CountryEnabled)
            {
                RuleFor(x => x.CountryId)
                    .NotNull()
                    .WithMessage(localizationService.GetResource("Address.Fields.Country.Required"));
                RuleFor(x => x.CountryId)
                    .NotEqual("")
                    .WithMessage(localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                {
                    //does selected country has states?
                    var countryId = !String.IsNullOrEmpty(x.CountryId) ? x.CountryId : "";
                    var hasStates = stateProvinceService.GetStateProvincesByCountryId(countryId).Count > 0;
                    if (hasStates)
                    {
                        //if yes, then ensure that state is selected
                        if (String.IsNullOrEmpty(x.StateProvinceId))
                        {
                            return false;
                        }
                    }
                    return true;
                }).WithMessage(localizationService.GetResource("Address.Fields.StateProvince.Required"));
                RuleFor(x => x.DistrictName)
                   .NotNull()
                   .WithMessage("Mahalle bilgisi gereklidir.");
                RuleFor(x => x.DistrictName)
                    .NotEqual("")
                    .WithMessage("Mahalle bilgisi gereklidir.");
                RuleFor(x => x.StreetName)
                    .NotNull()
                    .WithMessage("Sokak bilgisi gereklidir.");
                RuleFor(x => x.StreetName)
                    .NotEqual("")
                    .WithMessage("Sokak bilgisi gereklidir.");
                RuleFor(x => x.BuildingNumber)
                    .NotNull()
                    .WithMessage("Dış kapı numarası bilgisi gereklidir.");
                RuleFor(x => x.BuildingNumber)
                    .NotEqual("")
                    .WithMessage("Dış kapı numarası bilgisi gereklidir.");
            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                RuleFor(x => x.Company).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Company.Required"));
            }
            if (addressSettings.VatNumberRequired && addressSettings.VatNumberEnabled)
            {
                //RuleFor(x => x.VatNumber).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.VatNumber.Required"));
                RuleFor(x => x.VatNumber).NotEmpty().WithMessage("Vergi numarası gereklidir").Must(ValidateTaxNumber).When(x => x.SelectedCompany == "Legal").WithMessage("Geçersiz vergi numarası");
                RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("TC Kimlik numarası gereklidir").Must(ValidateIdentityNumber).When(x => x.SelectedCompany == "Real").WithMessage("Geçersiz kimlik numarası");
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.Address1).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress.Required"));
            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.Address2).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.StreetAddress2.Required"));
            }
            if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.ZipPostalCode.Required"));
            }
            if (addressSettings.CityRequired && addressSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.City.Required"));
            }
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Phone.Required"));
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                RuleFor(x => x.FaxNumber).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Fax.Required"));
            }
        }

        private bool ValidateIdentityNumber(string identityNumber)
        {
            if (identityNumber == null)
                return false;

            if (!IsNumeric(identityNumber))
                return false;

            if (identityNumber.Length != 11)
                return false;

            long tcNo = Convert.ToInt64(identityNumber);

            int[] digits = new int[11];
            int[] evenSumDigits = new int[5];
            int[] oddSumDigits = new int[5];
            int sumEven = 0;
            int sumOdd = 0;

            for (int i = 0; i < 11; i++)
            {
                digits[i] = Convert.ToInt32(tcNo.ToString()[i].ToString());
            }

            for (int i = 0; i < 5; i++)
            {
                evenSumDigits[i] = digits[i * 2];
                sumEven += evenSumDigits[i];

                if (i < 4)
                {
                    oddSumDigits[i] = digits[i * 2 + 1];
                    sumOdd += oddSumDigits[i];
                }
            }

            int tenthDigit = ((sumEven * 7) - sumOdd) % 10;
            int eleventhDigit = (sumEven + sumOdd + digits[9]) % 10;

            return digits[10] == eleventhDigit && digits[9] == tenthDigit;
        }

        private bool IsNumeric(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private bool ValidateTaxNumber(string taxNumber)
        {
            if (taxNumber == null)
                return false;

            // Vergi numarası uzunluğunun 10 olması gerekiyor.
            if (taxNumber.Length != 10)
            {
                return false;
            }

            // İlk rakamın sıfır olması gerekiyor.
            if (taxNumber[0] == '0')
            {
                return false;
            }
            foreach (char digit in taxNumber)
            {
                if (!char.IsDigit(digit))
                    return false;
            }

            // Son rakamın çift olması gerekiyor.
            //int lastDigit = int.Parse(taxNumber[9].ToString());
            //if (lastDigit % 2 != 0)
            //{
            //    return false;
            //}

            // Vergi numarasının kontrol basamağı doğru olmalıdır.
            //int[] taxNumberArray = new int[10];
            //for (int i = 0; i < 10; i++)
            //{
            //    taxNumberArray[i] = int.Parse(taxNumber[i].ToString());
            //}
            //int controlDigit = (7 * (taxNumberArray[0] + taxNumberArray[2] + taxNumberArray[4] + taxNumberArray[6]))
            //                   - (3 * (taxNumberArray[1] + taxNumberArray[3] + taxNumberArray[5] + taxNumberArray[7]));
            //controlDigit = controlDigit % 10;
            //if (controlDigit != taxNumberArray[8])
            //{
            //    return false;
            //}

            // Tüm kontroller geçildi, vergi numarası doğru.
            return true;
        }
    }
}