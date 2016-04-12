(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Stages', Stages);

  Stages.$inject = ['dataservice','warning'];

  /* @ngInject */
  function Stages(dataservice, warning) {
    var vm = this;
    vm.stages = [];
    vm.stage = {};

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
        .get()
        .then(function (data) {
          vm.stages = data;
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
      warning.warn(["This stage may be connected to other things, and deleting it will remove it from these as well.", "Are you sure you want to continue?"]).then(function(){
        dataservice.stages
          .remove(id)
          .then(function () {
            _.remove(vm.stages, function (s) {
              return s.id == id;
            });
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
          target.errMsg = "Stage with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();
