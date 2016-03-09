(function() {
  'use strict';

  angular
      .module('CRM')
      .controller('People', People);

  People.$inject = ['dataservice'];

  /* @ngInject */
  function People(dataservice) {
    var vm = this;

    vm.people = [];
    vm.pagination = {
      pageSize: 5,
      page: 1
    };
    vm.itemCount = 0;

    vm.getPeople = getPeople;

    activate();

    function activate() {
      getPeople();
    }

    function getPeople() {
      dataservice.people
        .get({query: vm.pagination})
        .then(function (data) {
          vm.itemCount = data.itemCount;
          vm.people = data.data;
        });
    }
  }
})();
