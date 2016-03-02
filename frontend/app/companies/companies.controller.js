(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Companies', Companies);

  Companies.$inject = ['dataservice'];

  /* @ngInject */
  function Companies(dataservice) {
    var vm = this;
    vm.companies = [];
    vm.pagination = {
      pageSize: 5,
      page: 1
    };
    vm.itemCount = 0;

    vm.getCompanies = getCompanies;

    activate();

    function activate() {
      getCompanies();
    }

    function getCompanies() {
      dataservice.companies
        .getAll({query: vm.pagination})
        .then(function (data) {
          vm.itemCount = data.itemCount;
          vm.companies = data.data;
        });
    }
  }
})();



