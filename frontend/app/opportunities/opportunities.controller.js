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

    activate();

    function activate() {
      getOpportunities();
    }

    function getOpportunities() {
      dataservice.opportunities
        .getAll()
        .then(function (data) {
          vm.opportunities = _.map(data, function (o) {
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
