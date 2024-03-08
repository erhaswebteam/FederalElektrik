$(document).ready(function() {

  //Anasayfa slider

  $(".home-slider").slick({
    autoplay: true,
    fade: false,
    dots: true,
    arrows: false,
    slidesToShow: 1,
    autoplaySpeed: 4000,
    lazyLoad: 'progressive'
  });

  $(".slider").slick({
    autoplay: true,
    fade: false,
    arrows: false,
    dots: true,
    slidesToShow: 1,
    lazyLoad: 'progressive'
  });


  $(".service-slider").slick({
    autoplay: true,
    fade: false,
    dots: true,
    arrows: false,
    slidesToShow: 1,
    lazyLoad: 'progressive'
  });

  $(".why-slider").slick({
    autoplay: true,
    fade: false,
    dots: true,
    arrows: false,
    slidesToShow: 1,
    lazyLoad: 'progressive'
  });

  $("#ref-slider-1").slick({
    autoplay: true,
    fade: false,
    dots: false,
    arrrows: false,
    autoplaySpeed: 1000,
    slidesToShow: 5,
    swipeToSlide: true,
    lazyLoad: 'progressive',
    responsive: [{
        breakpoint: 1200,
        settings: {
          slidesToShow: 4
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 4
        }
      },
      {
        breakpoint: 768,
        settings: {
          slidesToShow: 3
        }
      },
      {
        breakpoint: 576,
        settings: {
          slidesToShow: 2
        }
      }
    ]
  });

  $("#ref-slider-2").slick({
    autoplay: true,
    fade: false,
    dots: false,
    arrrows: false,
    autoplaySpeed: 4000,
    slidesToShow: 3,
    swipeToSlide: true,
    lazyLoad: 'progressive',
    responsive: [{
        breakpoint: 1200,
        settings: {
          slidesToShow: 2
        }
      },
      {
        breakpoint: 992,
        settings: {
          slidesToShow: 3
        }
      },
      {
        breakpoint: 768,
        settings: {
          slidesToShow: 2
        }
      },
      {
        breakpoint: 576,
        settings: {
          slidesToShow: 1
        }
      }
    ]
  });

  $(".dt-link").click(function(e) {
    event.preventDefault();
    $(".dt-link").removeClass("active");
    $(this).addClass("active");
    var dt_deger = $(this).attr("data-deger");

    $(".dt-tab").fadeOut(300);

    setTimeout(function(){
      $("#" + dt_deger).fadeIn();
    }, 300);
  })

  $(".has-sub").click(function() {
    $(this).toggleClass("active");
    $(".has-sub").removeClass("active");
    $(this).next().slideToggle();
  });

  $(".faq__title").click(function() {
    $(this).toggleClass("active");
    $(this).parent().find(".faq__text").slideToggle();
  });

  var wW = $(window).width();
  var cW = $(".container").width();
  var bosluk = ((wW - cW) / 2);

  // $(".home-slider-wrapper .form").css("right", +bosluk + "px");
  // $(".home-slider .slick-dots").css("padding-left", +bosluk + "px");


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

  $(".mobil-menu-link").click(function() {
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

  $(".alt-menu .tab-link").hover(function(e) {
    e.preventDefault();
    var currentTab = $(this).attr("opentab");
    $(this).parent(".tab-nav").find(".tab-link").removeClass("active");
    $(this).addClass("active");

    $(".tab-panel[tabname=" + currentTab + "]").parent(".tabs").find(".tab-panel").removeClass("tab-active");
    $(".tab-panel[tabname=" + currentTab + "]").addClass("tab-active");
  });
});


$.fn.isInViewport = function() {
  var n = $(this).offset().top,
    i = n + $(this).outerHeight(),
    t = $(window).scrollTop(),
    r = t + $(window).height();
  return i > t && n < r
};

$(window).on("resize scroll", function() {
  if ($(".footer").isInViewport()) {
    $(".footer-nav").removeClass("footer--fixed");
    $(".footer-btn").removeClass("btn--fixed");
  } else {
    $(".footer-nav").addClass("footer--fixed");
    $(".footer-btn").addClass("btn--fixed");
  }
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
