(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('User', User);

  User.$inject = ['dataservice', "$log", "$stateParams", '$state'];
  function User(dataservice, $log, $stateParams, $state){
    var vm = this;
    vm.user = {};
    vm.roles = ["Standard", "Executive", "Super"];
    vm.editing = true;

    vm.save = save;
    vm.cancel = cancel;
    vm.remove = remove;

    activate();

    function activate() {
      if($stateParams.id !== "new")
        getUser($stateParams.id);
      else {
        vm.editing = false;
        vm.user = {
          role: vm.roles[0]
        };
      }
    }

    function getUser(id) {
      dataservice.users
        .getById(id)
        .then(function (data) {
          vm.user = data;
          return vm.user;
        });
    }

    function save() {
      if(vm.editing){
        dataservice.users
          .update($stateParams.id, vm.user)
          .then(function () {
            $state.go("Users");
          });
      }
      else{
        dataservice.users
          .create($stateParams.id, vm.user)
          .then(function () {
            $state.go("Users");
          });
      }
    }

    function cancel() {
      $state.go("Users");
    }

    function remove() {
      dataservice.users
        .remove($stateParams.id)
        .then(function () {
          $state.go("Users");
        });
    }
  }
})();
