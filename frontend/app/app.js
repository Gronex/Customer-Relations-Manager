(function () {
  'use strict';

  angular.module('app', [
    // Angular modules
    'ngAnimate',
    // Custom modules

    // 3rd Party Modules
    'ui.router',
    'ui.materialize'
  ])
  .config(function($stateProvider, $urlRouterProvider) {
    //
    // For any unmatched url, redirect to /state1
    $urlRouterProvider.otherwise("/state1");
    //
    // Now set up the states
    $stateProvider
      .state('state1', {
        url: "/state1",
        templateUrl: "view/app/views/state1.html"
      })
      .state('state1.list', {
        url: "/list",
        templateUrl: "view/app/views/state1.list.html",
        controller: function($scope) {
          $scope.items = ["A", "List", "Of", "Items"];
        }
      })
      .state('state2', {
        url: "/state2",
        templateUrl: "view/app/views/state2.html"
      })
      .state('state2.list', {
        url: "/list",
        templateUrl: "view/app/views/state2.list.html",
        controller: function($scope) {
          $scope.things = ["A", "Set", "Of", "Things"];
        }
      });
  });
})();
