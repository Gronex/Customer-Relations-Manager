(function(){
  'use strict';

  angular
    .module('CRM')
    .directive('crmModelstateFeedback', ModelstateFeedback);

  function ModelstateFeedback(){
    var directive = {
      restrict: 'EA',
      templateUrl: 'view/app/components/modelstate-feedback.html',
      scope: {
        modelState: "="
      },
      controller: Controller,
      controllerAs: 'vm',
      bindToController: true
    };
    return directive;
  }

  Controller.$inject = ['$scope'];
  function Controller($scope){
    var vm = this;
    vm.alarms = [];
    vm.closeAlert = closeAlert;

    activate();

    function activate(){
    }

    $scope.$watch('vm.modelState', function(newVal){
      if(newVal !== undefined){
        vm.alarms = _.map(newVal.modelState, function(prop){ return prop;});
      } else{
        vm.alarms = [];
      }
    });

    function closeAlert(key){
      vm.alarms.splice(key, 1);
    }
  }
})();
