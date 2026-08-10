<template>
  <li v-on:click.stop.prevent class="uk-text-center uk-text-truncate">
    <a href="#">{{ $t('obs.navigation_confirmation') }}</a>
  </li>
</template>

<script>
  export default {

    props: ['index'],

  }
</script>
