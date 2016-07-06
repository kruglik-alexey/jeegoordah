import React from 'react'
import Total from './total/total'
import NavBar from './navBar'
import P2PList from './p2pList'

export default function () {
    return (
        <div>
            <NavBar />
            <div className="container-fluid main-container">
                <div className="row">
                    <div className="col-xs-3">
                        <Total />
                    </div>
                    <div className="col-xs-1"></div>
                    <div className="col-xs-8">
                        <P2PList />
                    </div>
                </div>
            </div>
        </div>
    );
}