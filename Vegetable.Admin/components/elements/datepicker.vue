<template>
 <v-row wrap>
     <v-col>
        <v-menu
          ref="menu"
          :close-on-content-click="false"
          v-model="menu"
          :nudge-right="40"
          lazy
          transition="scale-transition"
          offset-y
          full-width
          max-width="290px"
          min-width="290px"
        >
          <v-text-field
            slot="activator"
            v-model="displayDate"
            :label="title"
            persistent-hint
            prepend-icon="event"
            readonly
          ></v-text-field>
          <v-date-picker v-model="date" no-title @input="menu = false" @change="selectDate(date)"></v-date-picker>
        </v-menu>   
      </v-col>  
    </v-row>
</template>

<script>
 let moment = require('moment')
 export default {
   data () {
     return {
       time: null,
       menu: false,
       date: null,
       mutableDate: this.currentDate
 
     }
   },

   props: {
     currentDate: {},
     dateFormat: {},
     title: {}
   },
   computed: {
     displayDate () {
       return moment(this.mutableDate).format(this.dateFormat)
     }
   },
 
   methods: {
     selectDate (date) {
       var vm = this
       vm.$refs.menu.save(this.date)
       this.mutableDate = this.date
       this.$emit('datechange', date)
     }
   }
 }
</script>