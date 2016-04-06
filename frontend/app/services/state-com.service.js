(function(){
  'use strict';

  angular
    .module("CRM")
    .factory("stateCom", service);

  function service(){

    var $workers = {};
    var $args = {};
    var $lastArgs = {};

    return {
      setupFunction: setupFunction,
      setArgs: setArgs,
      invoke: invoke,
      resend: resend,
      isDefined: isDefined
    };

    function isDefined(name){
      return $workers[name] !== undefined;
    }

    function setupFunction(name, func){
      if(typeof(func) === "function")
        $workers[name] = func;
      else
        throw "argument has to be a function";
    }

    function setArgs(name, args){
      $args[name] = args;
    }

    function invoke(name, args){
      var tempArgs = _.cloneDeep($args[name]);
      var worker = $workers[name];
      if(worker === undefined) throw "Worker '" + name + "' not defined, call 'setupFunction' first";
      $lastArgs[name] = tempArgs;
      return $workers[name](_.merge(tempArgs, args));
    }

    function resend(name){
      if($lastArgs[name] !== undefined)
        invoke(name, $lastArgs[name]);
    }
  }
})();
