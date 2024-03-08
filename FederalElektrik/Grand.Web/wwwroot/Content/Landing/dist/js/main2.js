$(document).ready(function() {

  //Anasayfa slider

  var $status = $('.anasayfa-nav');
  var $slickElement = $('.anasayfa-slider');

  $slickElement.on('init reInit afterChange', function(event, slick, currentSlide, nextSlide) {
    var i = (currentSlide ? currentSlide : 0) + 1;

    $status.html('<span class="as-n-1">' + '0' + i + '</span>' + '<span class="as-n-2">' + '0' + slick.slideCount + '</span>');


    var sdW = $(".anasayfa-slider .slick-dots").width();

    $(".as-n-2").css("margin-left", + sdW + 25 + "px");
  });

  $slickElement.slick({
    autoplay: true,
    fade: false,
    dots: true,
    prevArrow: $('.prevAS'),
    nextArrow: $('.nextAS'),
    slidesToShow: 1,
    lazyLoad: 'progressive'
  });

  $(".atiklar-slider").slick({
    autoplay: true,
    fade: true,
    dots: true,
    prevArrow: $('.prevAS2'),
    nextArrow: $('.nextAS2'),
    slidesToShow: 1,
    lazyLoad: 'progressive'
  });


  //Mobil menü btn
  $("#btnMobil").click(function() {
    $("#btnMobil").toggleClass("is-active");
    $(".mobil").toggleClass("mobil-active");
    $("body").toggleClass("noscroll");
    $(".mobil-bg").toggleClass("arka-active");
  });

  $(".mobil-bg").click(function() {
    $("#btnMobil").toggleClass("is-active");
    $(".mobil").toggleClass("mobil-active");
    $("body").toggleClass("noscroll");
    $(".mobil-bg").toggleClass("arka-active");
  });

  $(".mobil-alt-menu-var").click(function() {
    $(this).next().slideToggle();
  })

  $(".mobil-menu-link").click(function(){
    $(".mobil-menu-link").removeClass("active");
    $(this).addClass("active");
  });

  //tablar
  $(".tab-link").click(function(e) {
    e.preventDefault();
    var currentTab = $(this).attr("opentab");
    $(this).parent(".tab-nav").find(".tab-link").removeClass("active");
    $(this).addClass("active");

    $(".tab-panel[tabname=" + currentTab + "]").parent(".tabs").find(".tab-panel").removeClass("tab-active");
    $(".tab-panel[tabname=" + currentTab + "]").addClass("tab-active");
  });
});





//alt menu
// const $menu = $('.alt-menu');
//
// $(document).mouseup(function(e) {
//   if (!$menu.is(e.target) &&
//     $menu.has(e.target).length === 0) {
//     $(".arka-bg").fadeOut(100);
//     $menu.fadeOut(100);
//   }
// });
//
// $('.alt-menu-click').click(function() {
//   $menu.fadeIn(100);
//   $(".arka-bg").fadeIn(100);
// });
