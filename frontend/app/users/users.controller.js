(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Users', Users);

  Users.$inject = ['dataservice', '$log'];
  function Users(dataservice){
    var vm = this;
    vm.users = [];
    vm.pagination = {
      pageSize: 5,
      page: 1
    };
    vm.itemCount = 0;

    vm.getUsers = getUsers;

    activate();

    function activate() {
      getUsers();
    }

    function getUsers() {
      return dataservice.users
        .getAll({query: vm.pagination})
        .then(function (data) {
          vm.itemCount = data.itemCount;
          vm.users = data.data;
          return vm.users;
        });
    }

  }
})();
