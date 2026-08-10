var fs = require('fs')
var {Nuxt, Builder}  = require('nuxt')
var resolve = require('path').resolve
var app = require('express')()
var host = process.env.HOST || '127.0.0.1'
var port = process.env.PORT || 3000

app.set('port', port)

// Import and Set Nuxt.js options
//let config = require('./nuxt.config.js') will not work on azure because relative lookup is troublesome at times
var rootDir = resolve('.')
var nuxtConfigFile = resolve(rootDir, 'nuxt.config.js')

var options = {}
if (fs.existsSync(nuxtConfigFile)) {
  options = require(nuxtConfigFile)
}
if (typeof options.rootDir !== 'string') {
  options.rootDir = rootDir
}
options.dev = false // Force production mode (no webpack middleware called)

var nuxt = new Nuxt(options)
nuxtConfigFile.dev = !(process.env.NODE_ENV === 'production');
app.use(nuxt.render)

if (nuxtConfigFile.dev) {
  const builder = new Builder(nuxt)
  builder.build()
}

// Listen the server
app.listen(port, host)
console.log('Server listening on ' + host + ':' + port) // eslint-disable-line no-console