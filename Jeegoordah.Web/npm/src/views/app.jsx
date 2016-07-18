import React from 'react'
import NavBar from './navBar'

export default function (props) {
    return (
        <div className="main-container">
            <NavBar />
            {props.children}
        </div>
    );
}