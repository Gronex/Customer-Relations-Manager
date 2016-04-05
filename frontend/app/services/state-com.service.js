(function(){
  'use strict';

  angular
    .module("CRM")
    .factory("stateCom", service);

  function service(){

    var $worker = function(args){
      throw "Not set, call 'SetupFunction' first.";
    };
    var $args = undefined;

    return {
      setupFunction: setupFunction,
      setArgs: setArgs,
      invoke: invoke
    };

    function setupFunction(func){
      if(typeof(func) === "function")
        $worker = func;
      else
        throw "argument has to be a function";
    }

    function setArgs(args){
      $args = args;
    }

    function invoke(args){
      var tempArgs = _.cloneDeep($args);
      return $worker(_.merge(tempArgs, args));
    }
  }
})();
