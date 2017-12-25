var imgRemaining = 3;
var fadeInDuration = 150;

function onLoadImg() {
    if ((--imgRemaining) <= 0) {
        $("#indexCarousel").fadeIn(fadeInDuration);
    }
}
