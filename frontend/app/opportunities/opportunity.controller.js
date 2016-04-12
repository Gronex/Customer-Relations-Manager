(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Opportunity', Opportunity);

  Opportunity.$inject = ['dataservice', '$stateParams', '$state', 'warning'];

  /* @ngInject */
  function Opportunity(dataservice, $stateParams, $state, warning) {
    var vm = this;

    vm.opportunity = {};
    vm.stages = [];
    vm.departments = [];
    vm.categories = [];
    vm.employees = [];

    vm.save = save;
    vm.remove = remove;
    vm.updateCompany = updateCompany;
    vm.updateContact = updateContact;
    vm.stageSelected = stageSelected;
    activate();

    function activate() {
      if($state.is("Opportunities.edit")){
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
        .get($stateParams.id)
        .then(function (data) {
          data.expectedClose = new Date(data.expectedClose);
          data.startDate = new Date(data.startDate);
          data.endDate = new Date(data.endDate);

          vm.opportunity = data;
          if(data.company.id)
            getEmployees(data.company.id);
          return vm.opportunity;
        });
    }
    function save() {

      if(vm.editing){
        return dataservice.opportunities
        .update($stateParams.id, vm.opportunity)
        .then(function () {
          $state.go("Opportunities.list");
        }, handleRequestError);
      } else {
        return dataservice.opportunities
        .create(vm.opportunity)
        .then(function () {
          $state.go("Opportunities.list");
        }, handleRequestError);
      }
    }

    function remove() {
      return warning.warn(["You are about to delete the opportunity '" + vm.opportunity.name + "', this operation cannot be undone.", "Are you sure you want to continue?"]).then(function(){
        return dataservice.opportunities
          .remove($stateParams.id)
          .then(function () {
            $state.go("Opportunities.list");
          });
      });
    }

    function stageSelected() {
      vm.opportunity.percentage = vm.opportunity.stage.value;
    }

    function getStages() {
      return dataservice.stages
        .get()
        .then(function (data) {
          vm.stages = data;
        });
    }

    function getDepartments() {
      return dataservice.departments
        .get()
        .then(function (data) {
          vm.departments = data;
        });
    }

    function getCategories() {
      return dataservice.opportunityCategories
        .get()
        .then(function (data) {
          vm.categories = data;
        });
    }

    function getEmployees(companyId){
      return dataservice.companyEmployees({companyId: companyId})
        .then(function(data){
          vm.opportunity.contactName = null;
          vm.opportunity.contactId = null;
          vm.employees = data;
        });
    }

    function updateCompany(company){
      vm.opportunity.company = company;
      getEmployees(company.id);
    }

    function updateContact(contact){
      vm.opportunity.contact = contact;
      vm.person = undefined;
    }

    function handleRequestError(err){
      if(err.status === 400){
        vm.modelState = err.data;
      }
    }
  }
})();
