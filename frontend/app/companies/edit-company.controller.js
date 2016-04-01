(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('EditCompany', Company);

  Company.$inject = ['company', 'employees', 'dataservice', '$stateParams', '$state'];

  /* @ngInject */
  function Company(company, employees, dataservice, $stateParams, $state) {
    var vm = this;
    vm.editing = false;

    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      if($state.is('Companies.edit')){
        vm.editing = true;
      }
      vm.company = company;
      vm.employees = employees;
    }

    function save() {
      if(vm.editing){
        dataservice.companies
        .update($stateParams.id, vm.company)
        .then(function () {
          $state.go("Companies.list");
        }, handleRequestError);
      } else {
        dataservice.companies
        .create(vm.company)
        .then(function () {
          $state.go("Companies.list");
        }, handleRequestError);
      }
    }

    function remove() {
      dataservice.companies
        .remove($stateParams.id)
        .then(function () {
          $state.go("Companies.list");
        });
    }


    function handleRequestError(err){
      if(err.status === 400){
        vm.modelState = err.data;
      }
    }
  }
})();
