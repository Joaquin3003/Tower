mergeInto(LibraryManager.library, {
    ReloadPage: function () {
        window.location.reload();
    },
    ExitFullscreen: function () {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
});