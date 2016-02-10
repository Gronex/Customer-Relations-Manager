(function() {
  'use strict';

  angular
    .module('app')
    .controller('User', User);

  User.$inject = ['dataservice', "$log", "$stateParams", '$state'];
  function User(dataservice, $log, $stateParams, $state){
    var vm = this;
    vm.user = {};
    vm.roles = ["Standard", "Executive", "Super"];

    vm.save = save;
    vm.cancel = cancel;
    vm.editing = true;

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
      dataservice.getUser(id)
        .then(function (data) {
          vm.user = data;
          return vm.user;
        });
    }

    function save() {
      if(vm.editing){
        dataservice.updateUser($stateParams.id, vm.user)
        .then(function () {
          $state.go("Users");
        });
      }
      else{
        dataservice.createUser($stateParams.id, vm.user)
        .then(function () {
          $state.go("Users");
        });
      }
    }

    function cancel() {
      $state.go("Users");
    }
  }
})();
