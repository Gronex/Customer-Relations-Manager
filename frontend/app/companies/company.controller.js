(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Company', Company);

  Company.$inject = ['dataservice', '$stateParams', '$state'];

  /* @ngInject */
  function Company(dataservice, $stateParams, $state) {
    var vm = this;
    vm.editing = false;
    vm.company = {};
    vm.employees = [];

    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      if($stateParams.id !== 'new'){
        getCompany($stateParams.id);
        getEmployees($stateParams.id);
      }
    }

    function getCompany(id) {
      dataservice.companies
      .get(id)
      .then(function (data) {
        vm.company = data;
        vm.editing = true;
      });
    }

    function getEmployees(id){
      dataservice
        .companyEmployees({companyId: id})
        .then(function(data){
          vm.employees = data;
        });
    }

    function save() {
      if(vm.editing){
        dataservice.companies
        .update($stateParams.id, vm.company)
        .then(function () {
          $state.go("Companies");
        }, handleRequestError);
      } else {
        dataservice.companies
        .create(vm.company)
        .then(function () {
          $state.go("Companies");
        }, handleRequestError);
      }
    }

    function remove() {
      dataservice.companies
        .remove($stateParams.id)
        .then(function () {
          $state.go("Companies");
        });
    }


    function handleRequestError(err){
      if(err.status === 400){
        vm.modelState = err.data;
      }
    }
  }
})();
