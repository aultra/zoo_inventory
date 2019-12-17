import { combineReducers } from 'redux';

import appReducer from 'containers/App/reducer';
import inventoryReducer from 'components/UI/AppScreen/reducer';
import descriptionReducer from 'components/Inventory/DescriptionBox/reducer';

export default () =>
  combineReducers({
    app: appReducer,
    inventory: inventoryReducer,
    description: descriptionReducer,
  });
