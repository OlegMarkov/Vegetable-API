module.exports = {
  /*
  ** Headers of the page
  */
  head: {
    title: 'obs',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: 'online booking site' }
    ],
    script: [
      { src: 'https://cdnjs.cloudflare.com/ajax/libs/uikit/3.0.0-beta.42/js/uikit.min.js' },
      { src: 'https://cdnjs.cloudflare.com/ajax/libs/uikit/3.0.0-beta.42/js/uikit-icons.min.js'}
    ],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      { rel: 'stylesheet', href: 'https://cdnjs.cloudflare.com/ajax/libs/uikit/3.0.0-beta.42/css/uikit.min.css' }
    ]
  },
  /*
  ** Customize the progress bar color
  */
  loading: { color: '#3B8070' },

  plugins: ['~/plugins/i18n.js', {src: '~/plugins/logger.js'}],

  modules: ['@nuxtjs/dotenv'],

  /*
  ** Build configuration
  */
  build: {
    /*
    ** Run ESLint on save
    */

    extend (config, { isDev, isClient }) {
      if (isDev && isClient) {
        config.module.rules.push({
          enforce: 'pre',
          test: /\.(js|vue)$/,
          loader: 'eslint-loader',
          exclude: /(node_modules)/
        })
      }
    },

    vendor: ['axios', 'underscore']

  },

  mode: 'spa',

  router: {
    extendRoutes (routes, resolve) {
      routes.push({
        name: 'obs-fallback',
        path: '*',
        component: resolve(__dirname, 'pages/obs.vue')
      })
    }
  }
}
