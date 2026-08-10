<template>
<div class="text-center">
     <v-btn fab  small color="deep-purple" 
     v-for="(weekday, index) in daysForShow" :key='index' 
     v-bind:class= "[ getActiveStatus(index) ? 'theme--dark deep-purple' : 'deep-purple lighten-5']" 
     @click="chooseDay(index)">
         {{weekday.dn}}
     </v-btn>
</div>
    
</template>

<script>

export default {
  data () {
    return {
      weekdaysValues: [{dn: 'M', order: '0', active: false},
        {dn: 'T', order: '1', active: false},
        {dn: 'W', order: '2', active: false},
        {dn: 'T', order: '3', active: false},
        {dn: 'F', order: '4', active: false},
        {dn: 'S', order: '5', active: false},
        {dn: 'S', order: '6', active: false}]
    }
  },

  computed:
  {
    daysForShow () {
      this.weekDays.forEach(x => {
        this.weekdaysValues[x].active = true
      })
      return this.weekdaysValues
    }
  },
  props: {
    weekDays: {
      type: Array,
      default: () => []
    }
  },
  methods: {
    getActiveStatus (index) {
      return this.weekdaysValues[index].active
    },

    chooseDay (index) {
      let current = this.weekdaysValues[index].active
      this.weekdaysValues[index].active = !current
      // let activeDays = this.weekdaysValues.filter(function (obj) {
      //   return obj.active === true
      // })
      // activeDays.forEach(x => this.usingWeekDays.push(x))
    }
  }
}
</script>



