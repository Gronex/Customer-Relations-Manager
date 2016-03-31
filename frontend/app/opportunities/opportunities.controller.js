(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Opportunities', Opportunities);

  Opportunities.$inject = ['dataservice'];

  /* @ngInject */
  function Opportunities(dataservice) {
    var vm = this;
    vm.opportunities = [];
    vm.pagination = {
      pageSize: 10,
      page: 1
    };
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
        selector: "startDate"
      },
      {
        label: "End",
        selector: "endDate"
      },
      {
        label: "Close",
        selector: "expectedClose"
      },
      {
        label: "Amount",
        selector: "amount"
      },
      {
        icon: "fa fa-pencil-square-o",
        type: "btn-link",
        link: "Opportunity"
      }
    ];

    vm.getOpportunities = getOpportunities;
    vm.sort = sort;

    activate();

    function activate() {
      getOpportunities();
    }

    function getOpportunities() {
      dataservice.opportunities
        .get({query: vm.pagination})
        .then(setupData);
    }

    function setupData(data) {
      vm.itemCount = data.itemCount;
      vm.opportunities = _.map(data.data, function (o) {
        o.startDate = new Date(o.startDate).toLocaleDateString();
        o.endDate = new Date(o.endDate).toLocaleDateString();
        o.expectedClose = new Date(o.expectedClose).toLocaleDateString();
        return o;
      });
      return data;
    }

    function sort(selector){
      var test = angular.copy(vm.pagination);
      test.orderBy = selector;
      dataservice.opportunities
        .get({query: test})
        .then(setupData);
    }

  }
})();
