(function() {
  'use strict';

  angular.module('CRM', [
    // Angular modules
    'ngAnimate',
    // Custom modules

    // 3rd Party Modules
    'ui.bootstrap',
    'ui.router',
    'LocalStorageModule',
    'ngTagsInput'
  ]).config(RouteConfig);

  function RouteConfig($stateProvider, $urlRouterProvider) {
    //
    // For any unmatched url, redirect to /
    $urlRouterProvider.otherwise("/");
    //
    // Now set up the states
    $stateProvider
      .state('Home', {
        url: "/?{fromDate:\\d{4}-\\d{2}-\\d{2}}&{toDate:\\d{4}-\\d{2}-\\d{2}}",
        templateUrl: "view/app/home/home.html",
        controller: "Home",
        controllerAs: "vm"
      })
      .state('Users', {
        url: "/users",
        templateUrl: "view/app/users/users.html",
        controller: 'Users',
        controllerAs: 'vm'
      })
      .state("User", {
        url: "/users/{id}",
        templateUrl: "view/app/users/user.html",
        controller: "User",
        controllerAs: 'vm'
      })
      .state('Activities', {
        url: "/activities",
        templateUrl: "view/app/activities/activities.html",
        controller: 'Activities',
        controllerAs: 'vm'
      })
      .state("Activity", {
        url: "/activities/{id}",
        templateUrl: "view/app/activities/activity.html",
        controller: "Activity",
        controllerAs: 'vm'
      })
      .state("UserGroups", {
        url: "/groups",
        templateUrl: "view/app/user-groups/user-groups.html",
        controller: "UserGroups",
        controllerAs: 'vm'
      })
      .state("Stages", {
        url: "/stages",
        templateUrl: "view/app/stages/stages.html",
        controller: "Stages",
        controllerAs: 'vm'
      })
      .state("Departments", {
        url: "/departments",
        templateUrl: "view/app/departments/departments.html",
        controller: "Departments",
        controllerAs: 'vm'
      })
      .state("ActivityCategories", {
        url: "/activity-categories",
        templateUrl: "view/app/activity-categories/activity-categories.html",
        controller: "ActivityCategories",
        controllerAs: 'vm'
      })
      .state("OpportunityCategories", {
        url: "/opportunity-categories",
        templateUrl: "view/app/opportunity-categories/opportunity-categories.html",
        controller: "OpportunityCategories",
        controllerAs: 'vm'
      })
      .state("Companies", {
        url: "/companies",
        templateUrl: "view/app/companies/companies.html",
        controller: "Companies",
        controllerAs: 'vm'
      })
      .state("People", {
        url: "/people",
        templateUrl: "view/app/people/people.html",
        controller: "People",
        controllerAs: 'vm'
      })
      .state("Person", {
        url: "/people/{id}",
        templateUrl: "view/app/people/person.html",
        controller: "Person",
        controllerAs: 'vm'
      })
      .state("Company", {
        url: "/companies/{id}",
        templateUrl: "view/app/companies/company.html",
        controller: "Company",
        controllerAs: 'vm'
      })
      .state("Opportunities", {
        url: "/opportunities",
        templateUrl: "view/app/opportunities/opportunities.html",
        controller: "Opportunities",
        controllerAs: 'vm'
      })
      .state("Opportunity", {
        url: "/opportunities/{id}",
        templateUrl: "view/app/opportunities/opportunity.html",
        controller: "Opportunity",
        controllerAs: 'vm'
      })
      .state("Login", {
        url: "/login",
        templateUrl: "view/app/login/login.html",
        controller: "Login",
        controllerAs: 'vm'
      })
      .state("Logout", {
        url: "/logout",
        controller: "Logout",
        controllerAs: 'vm'
      });
  }
})();
