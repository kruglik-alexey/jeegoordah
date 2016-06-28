import React from 'react'

export default class IndexView extends React.Component {
    render() {
        console.log(this.props);
        return (<div className="container"><h1>Foo</h1></div>);
    }
}