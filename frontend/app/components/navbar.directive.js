(function(){
  angular
    .module("CRM")
    .directive("crmNavbar", navbar);

  function navbar(){
    var directive = {
      templateUrl: "view/app/components/navbar.html",
      scope: {
        navbar: "="
      }
    };
    return directive;
  }
})();
