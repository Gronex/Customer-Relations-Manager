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
    vm.stages = [];
    vm.departments = [];
    vm.categories = [];

    vm.save = save;
    vm.remove = remove;
    vm.stageSelected = stageSelected;
    vm.updateCompany = updateCompany;
    activate();

    function activate() {
      if($stateParams.id !== "new"){
        getOpportunity()
        .then(function () {
          vm.editing = true;
        });
      }
      getStages();
      getDepartments();
      getCategories();
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
    function save() {

      if(vm.editing){
        return dataservice.opportunities
        .update($stateParams.id, vm.opportunity)
        .then(function () {
          $state.go("Opportunities");
        });
      } else {
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

    function getDepartments() {
      return dataservice.departments
        .getAll()
        .then(function (data) {
          vm.departments = data;
        });
    }

    function getCategories() {
      return dataservice.opportunityCategories
        .getAll()
        .then(function (data) {
          vm.categories = data;
        });
    }

    function updateCompany(company){
      vm.opportunity.company = company;
    }
  }
})();
