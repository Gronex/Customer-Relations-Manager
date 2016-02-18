(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Opportunity', Opportunity);

  Opportunity.$inject = ['dataservice', '$stateParams', '$state'];

  /* @ngInject */
  function Opportunity(dataservice, $stateParams, $state) {
    var vm = this;

    vm.opportunity = {};
    vm.companies = [];
    vm.stages = [];

    vm.save = save;
    vm.remove = remove;
    vm.companySelected = companySelected;
    vm.stageSelected = stageSelected;

    activate();

    function activate() {
      if($stateParams.id !== "new"){
        getOpportunity()
        .then(function () {
          vm.editing = true;
        });
      }
      getCompanies(); //TODO: call with query param on search
      getStages();
    }

    function getOpportunity() {
      return dataservice.opportunities
        .getById($stateParams.id)
        .then(function (data) {
          data.expectedClose = new Date(data.expectedClose);
          data.startDate = new Date(data.startDate);
          data.endDate = new Date(data.endDate);

          vm.opportunity = data;
          return vm.opportunity;
        });
    }

    function getCompanies() {
      return dataservice.companies
        .getAll()
        .then(function (data){
          vm.companies = data;
          return vm.companies;
        });
    }

    function save() {

      if(vm.editing){
        return dataservice.opportunities
        .update($stateParams.id, vm.opportunity)
        .then(function () {
          $state.go("Opportunities");
        });
      } else {
        vm.opportunity.company = vm.companies[0];
        return dataservice.opportunities
        .create(vm.opportunity)
        .then(function () {
          $state.go("Opportunities");
        });
      }
    }

    function remove() {
      return dataservice.opportunities
        .remove($stateParams.id)
        .then(function () {
          $state.go("Opportunities");
        });
    }

    function companySelected(item) {
      vm.company = undefined;
      vm.opportunity.company = item;
    }

    function stageSelected() {
      vm.opportunity.percentage = vm.opportunity.stage.value;
    }

    function getStages() {
      return dataservice.stages
        .getAll()
        .then(function (data) {
          vm.stages = data;
        });
    }
  }
})();
