<template>
<li>address</li>
</template>


<script>

export default{

props: ['index'],

  computed: {
  },

  methods: {
    addressSelected: function(address){
      this.store.commit('changeAddress', address);
    }
  }
}

</script>
