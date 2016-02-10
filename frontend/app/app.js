(function () {
  'use strict';

  angular.module('CRM', [
    // Angular modules
    'ngAnimate',
    // Custom modules

    // 3rd Party Modules
    'ui.router',
    'ui.materialize',
    'LocalStorageModule'
  ]).config(RouteConfig);

  function RouteConfig($stateProvider, $urlRouterProvider) {
    //
    // For any unmatched url, redirect to /
    $urlRouterProvider.otherwise("/");
    //
    // Now set up the states
    $stateProvider
      .state('Home', {
        url: "/"
      })
      .state('Users', {
        url: "/users",
        templateUrl: "view/app/users/users.html",
        controller: 'Users',
        controllerAs: 'vm'
      })
      .state("User", {
        url: "/users/:id",
        templateUrl: "view/app/users/user.html",
        controller: "User",
        controllerAs: 'vm'
      })
      .state("Login", {
        url: "/login",
        templateUrl: "view/app/login/login.html",
        controller: "Login",
        controllerAs: 'vm'
      });
  }
})();
