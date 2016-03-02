(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Users', Users);

  Users.$inject = ['dataservice', '$log'];
  function Users(dataservice){
    var vm = this;
    vm.users = [];

    activate();

    function activate() {
      getUsers();
    }

    function getUsers() {
      return dataservice.users
        .getAll()
        .then(function (data) {
          vm.users = data.data;
          return vm.users;
        });
    }

  }
})();
