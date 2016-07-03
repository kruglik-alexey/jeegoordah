import React from 'react'
import TotalView from './total'
import NavBarView from './navBar'

export default function () {
    return (
        <div>
            <NavBarView />
            <div className="container main-container">
                <TotalView />
            </div>
        </div>
    );
}