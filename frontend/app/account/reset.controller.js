(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("ResetPassword", Controller);

  Controller.$inject = ['$scope', '$stateParams', 'dataservice', '$state'];
  function Controller($scope, $stateParams, dataservice, $state){
    var vm = this;
    vm.errors = null;

    vm.send = send;

    activate();

    function activate(){
      vm.error = false;
      vm.model = {
        userName: $stateParams.userName,
        password: "",
        repeatPassword: ""
      };
    }

    $scope.$watch('vm.model', function(){
      if(!vm.model.password || !vm.model.repeatPassword){
        vm.error = true;
        return;
      }

      vm.error = !vm.model.password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/)
        || vm.model.password !== vm.model.repeatPassword;
    }, true);

    function send(){
      dataservice.account.create(vm.model, {id: "resetPassword", query: {code: $stateParams.code}})
        .then(function(result){
          $state.go("Login");
        }, function(err){
          vm.errors = err.data.message.split("\n");
        });
    }
  }
})();
