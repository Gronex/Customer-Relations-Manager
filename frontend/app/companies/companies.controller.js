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

    activate();

    function activate() {
      getCompanies();
    }

    function getCompanies() {
      dataservice.companies
        .getAll()
        .then(function (data) {
          vm.companies = data;
        });
    }
  }
})();
