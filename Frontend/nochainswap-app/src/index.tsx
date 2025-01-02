import React from 'react';
import ReactDOM from 'react-dom/client';
//import './bootstrap-bwhale.css';
import './bootstrap.css';
import 'react-loading-skeleton/dist/skeleton.css';
import App from './App';
import { BrowserRouter } from "react-router-dom";
import env from 'react-dotenv';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter basename={env.SITE_BASENAME}>
      <App />
    </BrowserRouter>
  </React.StrictMode>
);
