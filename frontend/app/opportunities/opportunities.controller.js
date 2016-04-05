(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Opportunities', Opportunities);

  Opportunities.$inject = ['opportunities', '$state'];

  /* @ngInject */
  function Opportunities(opportunities, $state) {
    var vm = this;
    vm.itemCount = 0;

    vm.headers = [
      {
        label: "Name",
        selector: "name"
      },
      {
        label: "Stage",
        selector: "stage.name"
      },
      {
        label: "Likelihood",
        selector: "percentage"
      },
      {
        label: "Company",
        selector: "companyName"
      },
      {
        label: "Owner",
        selector: "ownerName"
      },
      {
        label: "Start",
        selector: "startDate",
        format: "date"
      },
      {
        label: "End",
        selector: "endDate",
        format: "date"
      },
      {
        label: "Close",
        selector: "expectedClose",
        format: "date"
      },
      {
        label: "Amount",
        selector: "amount"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Opportunities.edit"
      }
    ];

    vm.getOpportunities = getOpportunities;

    activate();

    function activate() {
      vm.itemCount = opportunities.itemCount;
      vm.opportunities = opportunities.data;
      vm.query = opportunities.query;
    }

    function getOpportunities(sortParam) {
      _.merge(vm.query, {orderBy: sortParam});
      $state.go(".", vm.query);
    }
  }
})();
