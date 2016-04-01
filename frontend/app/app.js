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
        url: "/",
        templateUrl: "view/app/home/home.html",
        controller: "Home",
        controllerAs: "vm"
      })
      .state('Home.Dashboard', {
        url: "dashboard/?{fromDate:\\d{4}-\\d{2}-\\d{2}}&{toDate:\\d{4}-\\d{2}-\\d{2}}",
        templateUrl: "view/app/home/dashboard.html",
        controller: "Dashboard",
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

      .state("Activities", {
        url: "/activities",
        abstract: true,
        template: "<ui-view></ui-view>"
      })
      .state('Activities.list', {
        url: "",
        templateUrl: "view/app/activities/activities.html",
        controller: 'Activities',
        controllerAs: 'vm'
      })
      .state("Activities.new", {
        url: "/activities/new?{contact:int}&{company:int}",
        templateUrl: "view/app/activities/activity.html",
        controller: "Activity",
        controllerAs: 'vm',
        resolve: {
          person: function(dataservice, $stateParams){
            if($stateParams.contact)
              return dataservice.people.get($stateParams.contact);
            else
              return {};
          },
          company: function(dataservice, $stateParams){
            if($stateParams.company)
              return dataservice.companies.get($stateParams.company);
            else if($stateParams.contact)
              return dataservice.people.get($stateParams.contact)
              .then(function(result){
                return dataservice.companies.get(result.companyId);
              });
            else
              return {};
          },
          activity: function(){return {};}
        }
      })
      .state("Activities.edit", {
        url: "/activities/{id:int}",
        templateUrl: "view/app/activities/activity.html",
        controller: "Activity",
        controllerAs: 'vm',
        resolve: {
          person: function(){ return {}; },
          company: function(){ return {}; },
          activity: function(dataservice, $stateParams){
            return dataservice.activities.get($stateParams.id);
          }
        }
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

      .state("People", {
        url: "/people",
        abstract: true,
        template: "<ui-view></ui-view>"
      })
      .state("People.list", {
        url: "",
        templateUrl: "view/app/people/people.html",
        controller: "People",
        controllerAs: 'vm'
      })
      .state("People.new", {
        url: "/new",
        templateUrl: "view/app/people/edit-person.html",
        controller: "EditPerson",
        controllerAs: 'vm',
        resolve: {
          person: function(){return {};}
        }
      })
      .state("People.edit", {
        url: "/{id:int}/edit",
        templateUrl: "view/app/people/edit-person.html",
        resolve: {
          person: function(dataservice, $stateParams){
            var res = dataservice.people.get($stateParams.id);
            return res;
          }
        },
        controller: "EditPerson",
        controllerAs: 'vm'
      })
      .state("People.view", {
        url: "/{id:int}/view",
        templateUrl: "view/app/people/view-person.html",
        resolve: {
          person: function(dataservice, $stateParams){
            return dataservice.people.get($stateParams.id);
          },
          activities: function(dataservice, $stateParams){
            return dataservice.personActivities({personId: $stateParams.id});
          }
        },
        controller: "ViewPerson",
        controllerAs: "vm"
      })

      .state("Companies", {
        url: "/companies",
        abstract: true,
        template: "<ui-view></ui-view>"
      })
      .state("Companies.list", {
        url: "",
        templateUrl: "view/app/companies/companies.html",
        controller: "Companies",
        controllerAs: 'vm'
      })
      .state("Companies.new", {
        url: "/companies/new",
        templateUrl: "view/app/companies/edit-company.html",
        controller: "EditCompany",
        controllerAs: 'vm',
        resolve: {
          company: function(){ return {}; },
          employees: function(){ return []; }
        }
      })
      .state("Companies.view", {
        url: "/{id:int}/view",
        templateUrl: "view/app/companies/view-company.html",
        controller: "ViewCompany",
        controllerAs: 'vm',
        resolve: {
          company: function(dataservice, $stateParams){
            return dataservice.companies.get($stateParams.id);
          },
          employees: function(dataservice, $stateParams){
            return dataservice.companyEmployees({companyId: $stateParams.id});
          },
          activities: function(dataservice, $stateParams){
            return dataservice.companyActivities({companyId: $stateParams.id});
          }
        }
      })
      .state("Companies.edit", {
        url: "/{id:int}/edit",
        templateUrl: "view/app/companies/edit-company.html",
        controller: "EditCompany",
        controllerAs: 'vm',
        resolve: {
          company: function(dataservice, $stateParams){
            return dataservice.companies.get($stateParams.id);
          },
          employees: function(dataservice, $stateParams){
            return dataservice.companyEmployees({companyId: $stateParams.id});
          }
        }
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
      })
      .state("Activate", {
        url: "/account/activate?{email}&{code}",
        controller: "ActivateAccount",
        controllerAs: "vm",
        templateUrl: "view/app/account/activate.html"
      })
      .state("ForgotPassword", {
        url: "/account/forgotPassword?{email}",
        controller: "ForgotPassword",
        controllerAs: "vm",
        templateUrl: "view/app/account/forgot.html"
      })
      .state("ResetPassword", {
        url: "/account/resetpassword?{userName}&{code}",
        controller: "ResetPassword",
        controllerAs: "vm",
        templateUrl: "view/app/account/activate.html"
      });
  }
})();
