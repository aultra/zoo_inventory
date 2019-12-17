import Nui from '../../../util/Nui';

export const SET_INVENTORY = 'SET_INVENTORY';
export const SET_HOVER_ITEM = 'SET_HOVER_ITEM';
export const CLEAR_HOVER_ITEM = 'CLEAR_HOVER_ITEM';
export const MOVE_ITEM = 'MOVE_ITEM';
export const DROP_ITEM = 'DROP_ITEM';


export const setHoverItem = (slot, item, type, owner) => {
  return {
    type: SET_HOVER_ITEM,
    payload: {
      ownerFrom: owner,
      typeFrom: type,
      slotFrom: slot,
      item: item,
    },
  };
};

export const clearHoverItem = {
  type: CLEAR_HOVER_ITEM,
};

export const moveItem = (slot, type, item, owner) => {
  return dispatch => {
    const payload = {
      data: {
        ...item.data,
        ownerTo: owner,
        slotTo: slot,
        typeTo: type,
      },
    };
    Nui.send(MOVE_ITEM, payload);
    dispatch({
        type: MOVE_ITEM,
        payload: payload,
      },
    );
    dispatch(clearHoverItem);
  };
};

export const dropItem = (slot, type, item, owner) => {
  return dispatch => {
    const payload = {
      data: {
        slot: slot,
        type: type,
        owner: owner,
        ...item,
      },
    };
    Nui.send(DROP_ITEM, payload);
    dispatch({
        type: DROP_ITEM,
        payload: payload,
      },
    );
  };
};
