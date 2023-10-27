using Grand.Core.Configuration;

namespace Grand.Plugin.Payments.IPara
{
    public class IParaSettings : ISettings
    {
        public string DescriptionText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
    }
}
