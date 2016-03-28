(function(){
  'use strict';

  angular
    .module("CRM")
    .controller("ActivateAccount", Controller);

  Controller.$inject = ['$scope', '$stateParams', 'dataservice', '$state'];
  function Controller($scope, $stateParams, dataservice, $state){
    var vm = this;
    vm.errors = null;

    vm.send = send;

    activate();

    function activate(){
      vm.error = false;
      vm.model = {
        userName: $stateParams.email,
        password: "",
        repeatPassword: ""
      };
    }

    $scope.$watch('vm.model', function(){
      vm.error = !vm.model.password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/)
        || vm.model.password !== vm.model.repeatPassword;
    }, true);

    function send(){
      dataservice.account.create(vm.model, {id: "confirmEmail", query: {code: $stateParams.code}})
        .then(function(result){
          $state.go("home");
        }, function(err){
          vm.errors = err.data.message.split("\n");
        });
    }
  }
})();
