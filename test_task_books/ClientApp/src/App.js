import React, { Component } from 'react';
import DataTable from './components/DataTable'
import './custom.css'

export default class App extends Component {
  static displayName = App.name;
    
  render () {
      return (
          <div className="container">
              <DataTable />
          </div>
    );
  }
}
