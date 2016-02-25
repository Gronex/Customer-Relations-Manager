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
      vm.people = [
        {
          id: 1,
          firstName: "Test1",
          lastName: "Test",
          email: "test@test.com",
          phoneNumber: "123123"
        }
      ];
    }

    function getPeople() {
      dataservice.people
        .getAll()
        .then(function (data) {
          vm.people = data;
        });
    }
  }
})();
