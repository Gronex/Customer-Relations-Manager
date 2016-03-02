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

    activate();

    function activate() {
      getPeople();
    }

    function getPeople() {
      dataservice.people
        .getAll()
        .then(function (data) {
          vm.people = data.data;
        });
    }
  }
})();
