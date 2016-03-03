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
      pageSize: 1,
      page: 1
    };
    vm.itemCount = 0;

    vm.getOpportunities = getOpportunities;

    activate();

    function activate() {
      getOpportunities();
    }

    function getOpportunities() {
      dataservice.opportunities
        .getAll({query: vm.pagination})
        .then(function (data) {
          vm.itemCount = data.itemCount;
          vm.opportunities = _.map(data.data, function (o) {
            o.startDate = new Date(o.startDate).toLocaleDateString();
            o.endDate = new Date(o.endDate).toLocaleDateString();
            o.expectedClose = new Date(o.expectedClose).toLocaleDateString();
            return o;
          });
          return data;
        });
    }
  }
})();
