(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Stages', Stages);

  Stages.$inject = ['dataservice'];

  /* @ngInject */
  function Stages(dataservice) {
    var vm = this;
    vm.stages = [];
    vm.stage;

    vm.create = create;
    vm.save = save;
    vm.remove = remove;

    activate();

    function activate() {
      getStages();
      resetStage();
    }

    function getStages() {
      dataservice.stages
        .getAll()
        .then(function (data) {
          vm.stages = _.orderBy(data, ['value']);
        });
    }

    function create() {
      vm.stage.errMsg = undefined;
      dataservice.stages
        .create(vm.stage)
        .then(function (data) {
          vm.stages.push(data.data);
          resetStage();
        }, function (err) { handleError(err, vm.stage); });
    }
    function save(stage) {
      stage.errMsg = undefined;
      if(stage.saved || stage.saved == undefined) return;
      dataservice.stages
        .update(stage.id, stage)
        .then(function () {
          stage.saved = true;
          resetStage();
        }, function (err) { handleError(err, stage); });
    }
    function remove(id) {
      dataservice.stages
        .remove(id)
        .then(function () {
          _.remove(vm.stages, function (s) {
            return s.id == id;
          });
        });
    }

    function resetStage() {
      vm.stages = _.orderBy(vm.stages, ['value']);
      vm.stage = {
        value: 0,
        name: ""
      };
    }

    function handleError(err, target) {
      switch (err.status) {
        case 400:

          break;
        case 409:
          target.errMsg = "Group with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();
