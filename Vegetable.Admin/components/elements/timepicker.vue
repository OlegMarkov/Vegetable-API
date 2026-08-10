<template>
 <v-row wrap>
      <v-flex>
        <v-menu
          ref="menu"
          :close-on-content-click="false"
          v-model="menu"
          :nudge-right="40"
          :return-value.sync="time"
          transition="scale-transition"
          offset-y
          max-width="290px"
          min-width="290px"
        >
        <template v-slot:activator="{ on }">
          <v-text-field
            v-on="on"
            v-model="time"
            :label="title"           
             prepend-icon="access_time"
            readonly
          ></v-text-field>
        </template>
          <v-time-picker v-model="time" scrollable format="24hr" @change="onChangeTime(time)"></v-time-picker>
        </v-menu>
      </v-flex>
      
    </v-row>
</template>

<script>
  export default {
    data () {
      return {
        time: this.currentTime,
        menu: false,
        modal: false

      }
    },

    props: {
      currentTime: {},
      title: {}
    },

    computed: {
      getLabel () {
        return this.title
      }
    },

    methods: {
      onChangeTime (time) {
        var vm = this
        vm.$refs.menu.save(time)
        this.mutableTime = time
        this.$emit('timechange', time)
        this.$emit('update:currentTime', time)
      }
    }
  }
</script>