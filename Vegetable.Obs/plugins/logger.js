
export default (context) => {

  // Set logger instance on app
  context.logger = {
    info: function(message){
      console.log('info: ' + message);
    } ,
    warn: function(){
      console.log('warn: ' + message);
    },
    error: function(){
      console.log('error: ' + message);
    }
  }

}


