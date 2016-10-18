module ICanHasDotnetCore.Layout {

    class ViewModel {
        version = ICanHasDotnetCore.version;

        constructor(private $mdSidenav) {
        }

        toggleSidebar() {
            this.$mdSidenav('left').toggle();
        }

    }

    addAngularState("layout", null, ViewModel, "layout/layout.html", {});
}
