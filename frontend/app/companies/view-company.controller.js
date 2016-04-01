(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ViewCompany', Company);

  Company.$inject = ['company', 'employees', 'activities', '$stateParams'];

  /* @ngInject */
  function Company(company, employees, activities, $stateParams, $state) {
    var vm = this;
    vm.editing = false;

    activate();

    function activate() {
      vm.company = company;
      vm.company.id = $stateParams.id;
      vm.employees = employees;
      vm.activities = activities;
    }
  }
})();
