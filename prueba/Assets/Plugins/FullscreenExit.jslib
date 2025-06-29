mergeInto(LibraryManager.library, {
    ExitFullscreen: function () {
        if (document.fullscreenElement) {
            document.exitFullscreen();
        } else if (document.webkitFullscreenElement) { // Soporte para Safari
            document.webkitExitFullscreen();
        }
    }
});