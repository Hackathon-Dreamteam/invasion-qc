import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import GlobalStyles from './styles/GlobalStyles.tsx';
import { RouterProvider, createBrowserRouter } from 'react-router-dom';
import DashboardPage from './pages/dashboard/DashboardPage.tsx';
import AlertPage from './pages/alert/AlertPage.tsx';
import LoginPage from './pages/login/LoginPage.tsx';
import { setDefaultOptions } from 'date-fns/setDefaultOptions';
import { fr } from 'date-fns/locale/fr';
import SpeciesPage from './pages/species/SpeciesPage.tsx';
import SpeciesDetailsPage from './pages/species/SpeciesDetailsPage.tsx';
import './styles/main.css';
import AlertPublishPage from './pages/alert/AlertPublishPage.tsx';
import ObservationsDetailsPage from './pages/observations/ObservationsDetailsPage.tsx';

setDefaultOptions({ locale: fr });

const router = createBrowserRouter([
  {
    Component: App,
    children: [
      {
        path: '/dashboard',
        Component: DashboardPage
      },
      {
        path: '/species',
        Component: SpeciesPage
      },
      {
        path: '/species/:speciesId',
        Component: SpeciesDetailsPage
      },
      {
        path: '/observations/:observationId',
        Component: ObservationsDetailsPage
      },
      {
        path: '/alerts',
        Component: AlertPage
      },
      {
        path: '/alerts/:alertId',
        Component: AlertPublishPage
      }
    ],
    errorElement: <div>Error!</div>
  },
  {
    path: '/',
    Component: LoginPage
  }
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
  // <React.StrictMode>
  <>
    <GlobalStyles />
    <RouterProvider router={router} />
  </>
  // </React.StrictMode>
);

if (import.meta.hot) {
  import.meta.hot.dispose(() => router.dispose());
}
