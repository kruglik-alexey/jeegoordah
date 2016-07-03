import React from 'react'
import Total from './total/total'
import NavBar from './navBar'

export default function () {
    return (
        <div>
            <NavBar />
            <div className="container main-container">
                <Total />
            </div>
        </div>
    );
}