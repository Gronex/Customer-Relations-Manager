(function() {
  'use strict';

  angular
      .module('CRM')
      .directive('crmCompanyTypeahead', companyTypeahead);

  /* @ngInject */
  function companyTypeahead() {
    var directive = {
      restrict: 'EA',
      templateUrl: 'view/app/components/company-typeahead.html',
      scope: {
        isRequired: "=",
        companyName: "=",
        onSelect: "=",
        onRemove: "=",
        canCreate: "@"
      },
      controller: Controller,
      controllerAs: 'vm',
      bindToController: true
    };

    return directive;
  }

  Controller.$inject = ['dataservice'];

  /* @ngInject */
  function Controller(dataservice) {
    var vm = this;

    vm.companies = [];

    vm.companySelected = companySelected;

    activate();

    function activate() {
      getCompanies();
    }

    function getCompanies() {
      return dataservice.companies
        .get()
        .then(function (data){
          vm.companies = data.data;
          return vm.companies;
        });
    }

    function companySelected(item) {
      vm.company = undefined;
      if(typeof(vm.onSelect) === "function")
        vm.onSelect(item);
    }
  }
})();
